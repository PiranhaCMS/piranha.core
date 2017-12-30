/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Piranha.Manager;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PostEditModel : Piranha.Models.PostBase
    {
        #region Properties
        /// <summary>
        /// Gets/sets the post type.
        /// </summary>
        public Piranha.Models.PostType PostType { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<PageEditRegionBase> Regions { get; set; }

        /// <summary>
        /// Gets/sets the available categories.
        /// </summary>
        public IEnumerable<Data.Category> AllCategories { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IEnumerable<Data.Tag> AllTags { get; set; }

        /// <summary>
        /// Gets/sets the currently selected category.
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// Gets/sets the currently selected tags.
        /// </summary>
        public IList<string> SelectedTags { get; set; }
        #endregion

         /// <summary>
        /// Default constructor.
        /// </summary>
        public PostEditModel() {
            Regions = new List<PageEditRegionBase>();
            AllCategories = new List<Data.Category>();
            AllTags = new List<Data.Tag>();
            SelectedTags = new List<string>();
        }

        /// <summary>
        /// Gets the edit model for the post with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The post id</param>
        /// <returns>The post edit model</returns>
        public static PostEditModel GetById(IApi api, Guid id) {
            var post = api.Posts.GetById(id);
            if (post != null) {
                var model = Module.Mapper.Map<Piranha.Models.PostBase, PostEditModel>(post);
                model.PostType = api.PostTypes.GetById(model.TypeId);
                model.AllCategories = api.Categories.GetAll(post.BlogId);
                model.AllTags = api.Tags.GetAll(post.BlogId);
                model.SelectedCategory = post.Category.Slug;
                model.SelectedTags = post.Tags.Select(t => t.Slug).ToList();

                LoadRegions(post, model);

                return model;
            }
            throw new KeyNotFoundException($"No post found with the id '{id}'");
        }

        /// <summary>
        /// Refreshes the model after an unsuccessful save.
        /// </summary>
        public PostEditModel Refresh(IApi api) {
            if (!string.IsNullOrWhiteSpace(TypeId)) {
                PostType = api.PostTypes.GetById(TypeId);
                AllCategories = api.Categories.GetAll(BlogId);
                AllTags = api.Tags.GetAll(BlogId);
            }
            return this;
        }        

        /// <summary>
        /// Saves the page model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="publish">If the page should be published</param>
        /// <returns>If the page was successfully saved</returns>
        public bool Save(IApi api, bool? publish = null) {
            var post = api.Posts.GetById(Id);

            if (post == null)
                post = Piranha.Models.DynamicPost.Create(api, this.TypeId);

            Module.Mapper.Map<PostEditModel, Piranha.Models.PostBase>(this, post);
            SaveRegions(api, this, post);

            // Update category
            var category = api.Categories.GetBySlug(BlogId, SelectedCategory);
            if (category != null)
                post.Category = category;
            else post.Category = new Piranha.Models.Taxonomy() {
                Title = SelectedCategory
            };

            // Update tags
            post.Tags.RemoveAll(t => !SelectedTags.Contains(t.Slug));

            foreach (var selectedTag in SelectedTags) {
                if (!post.Tags.Any(t => t.Slug == selectedTag)) {
                    var tag = api.Tags.GetBySlug(BlogId, selectedTag);

                    if (tag != null) {
                        post.Tags.Add(new Piranha.Models.Taxonomy() {
                            Id = tag.Id,
                            Title = tag.Title,
                            Slug = tag.Slug
                        });
                    } else {
                        post.Tags.Add(new Piranha.Models.Taxonomy() {
                            Title = selectedTag
                        });
                    }
                }
            }                        

            if (publish.HasValue) {
                if (publish.Value && !post.Published.HasValue)
                    post.Published = DateTime.Now;
                else if (!publish.Value)
                    post.Published = null;
            }
            api.Posts.Save(post);
            Id = post.Id;

            return true;
        }

        /// <summary>
        /// Creates a new edit model with the given post typeparamref.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="postTypeId">The post type id</param>
        /// <param name="blogId">The blog id</param>
        /// <returns>The post edit model</returns>        
        public static PostEditModel Create(IApi api, string postTypeId, Guid blogId) {
            var type = api.PostTypes.GetById(postTypeId);

            if (type != null) {
                var post = Piranha.Models.DynamicPost.Create(api, postTypeId);
                var model = Module.Mapper.Map<Piranha.Models.PostBase, PostEditModel>(post);
                model.BlogId = blogId;
                model.PostType = type;
                model.AllCategories = api.Categories.GetAll(blogId);
                model.AllTags = api.Tags.GetAll(blogId);

                LoadRegions(post, model);

                return model;
            }
            throw new KeyNotFoundException($"No post type found with the id '{postTypeId}'");
        }

        /// <summary>
        /// Creates a new edit region model for the given region type and value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <param name="value">The region value</param>
        /// <returns>The edit model</returns>
        public static PageEditRegionBase CreateRegion(Piranha.Models.RegionType region, object value) {
            PageEditRegionBase editRegion;

            if (region.Collection) {
                editRegion = new PageEditRegionCollection();
            } else {
                editRegion = new PageEditRegion();
            }
            editRegion.Id = region.Id;
            editRegion.Title = region.Title ?? region.Id;
            editRegion.CLRType = editRegion.GetType().FullName;

            IList items = new List<object>();

            if (region.Collection)
                items = (IList)value;
            else items.Add(value);

            foreach (var item in items) {
                if (region.Fields.Count == 1) {
                    var itemTitle = "";

                    // Get the item title if this is a collection region.
                    if (region.Collection) {
                        if (item != null)
                            itemTitle = ((Extend.IField)item).GetTitle();
                        if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                            itemTitle = region.ListTitlePlaceholder;
                        else itemTitle = "Item";
                    }

                    var set = new PageEditFieldSet() {
                        new PageEditField() {
                            Id = region.Fields[0].Id,
                            Title = region.Fields[0].Title ?? region.Fields[0].Id,
                            CLRType = item.GetType().FullName,
                            Options = region.Fields[0].Options,
                            Value = (Extend.IField)item
                        }
                    };
                    set.ListTitle = itemTitle;
                    set.NoExpand = !region.ListExpand;

                    editRegion.Add(set);
                } else {
                    var fieldData = (IDictionary<string, object>)item;
                    var fieldSet = new PageEditFieldSet();

                    foreach (var field in region.Fields) {
                        if (fieldData.ContainsKey(field.Id)) {
                            // Get the item title if this is a collection region.
                            if (region.Collection) {
                                if (!string.IsNullOrWhiteSpace(region.ListTitleField) && field.Id == region.ListTitleField) {
                                    var itemTitle = "";

                                    if (fieldData[field.Id] != null)
                                        itemTitle = ((Extend.IField)fieldData[field.Id]).GetTitle();
                                    if (string.IsNullOrWhiteSpace(itemTitle) && !string.IsNullOrWhiteSpace(region.ListTitlePlaceholder))
                                        itemTitle = region.ListTitlePlaceholder;
                                    else if (string.IsNullOrWhiteSpace(itemTitle)) 
                                        itemTitle = "Item";

                                    fieldSet.ListTitle = itemTitle;
                                    fieldSet.NoExpand = !region.ListExpand;
                                }
                            }

                            fieldSet.Add(new PageEditField() {
                                Id = field.Id,
                                Title = field.Title ?? field.Id,
                                CLRType = fieldData[field.Id].GetType().FullName,
                                Options = field.Options,
                                Value = (Extend.IField)fieldData[field.Id]
                            });
                        }
                    }
                    editRegion.Add(fieldSet);
                }
            }
            return editRegion;
        }        

        /// <summary>
        /// Loads all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private static void LoadRegions(Piranha.Models.DynamicPost src, PostEditModel dest) {
            if (dest.PostType != null) {
                foreach (var region in dest.PostType.Regions) {
                    var regions = (IDictionary<string, object>)src.Regions;

                    if (regions.ContainsKey(region.Id)) {
                        var editRegion = CreateRegion(region, regions[region.Id]);
                        dest.Regions.Add(editRegion);
                    }
                }
            }
        }

        /// <summary>
        /// Saves all of the regions from the source model into the destination.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="src">The source</param>
        /// <param name="dest">The destination</param>
        private static void SaveRegions(IApi api, PostEditModel src, Piranha.Models.DynamicPost dest) {
            var modelRegions = (IDictionary<string, object>)dest.Regions;
            foreach (var region in src.Regions) {
                if (region is PageEditRegion) {
                    if (!modelRegions.ContainsKey(region.Id))
                        modelRegions[region.Id] = Piranha.Models.DynamicPost.CreateRegion(api, dest.TypeId, region.Id);

                    var reg = (PageEditRegion)region;

                    if (reg.FieldSet.Count == 1) {
                        modelRegions[region.Id] = reg.FieldSet[0].Value;
                    } else {
                        var modelFields = (IDictionary<string, object>)modelRegions[region.Id];

                        foreach (var field in reg.FieldSet) {
                            modelFields[field.Id] = field.Value;
                        }
                    }
                } else {
                    if (modelRegions.ContainsKey(region.Id)) {
                        var list = (Piranha.Models.IRegionList)modelRegions[region.Id];
                        var reg = (PageEditRegionCollection)region;

                        // At this point we clear the values and rebuild them
                        list.Clear();

                        foreach (var set in reg.FieldSets) {
                            if (set.Count == 1) {
                                list.Add(set[0].Value);
                            } else {
                                var modelFields = (IDictionary<string, object>)Piranha.Models.DynamicPost.CreateRegion(api, dest.TypeId, region.Id);

                                foreach (var field in set) {
                                    modelFields[field.Id] = field.Value;
                                }
                                list.Add(modelFields);
                            }

                        }
                    }
                }
            }
        }        
    }
}