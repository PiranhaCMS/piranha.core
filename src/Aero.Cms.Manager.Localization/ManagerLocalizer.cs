

using Microsoft.Extensions.Localization;


namespace Aero.Manager
{
    public class ManagerLocalizer
    {
        /// <summary>
        /// Gets/sets alias string resources.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Alias> Alias { get; private set; }

        /// <summary>
        /// Gets/sets comment string resources.
        /// </summary>
        /// <value></value>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Comment> Comment { get; private set; }

        /// <summary>
        /// Gets/sets content string resources.
        /// </summary>
        /// <value></value>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Content> Content { get; private set; }

        /// <summary>
        /// Gets/sets config string resources.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Config> Config { get; private set; }

        /// <summary>
        /// Gets/sets general string resources.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.General> General { get; private set; }

        /// <summary>
        /// Gets/sets security string resources.
        /// </summary>
        /// <value></value>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Security> Security { get; private set; }

        /// <summary>
        /// Gets/sets language string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Language> Language { get; private set; }

        /// <summary>
        /// Gets/sets media string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Media> Media { get; private set; }

        /// <summary>
        /// Gets/sets menu string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Menu> Menu { get; private set; }

        /// <summary>
        /// Gets/sets module string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Module> Module { get; private set; }

        /// <summary>
        /// Gets/sets page string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Page> Page { get; private set; }

        /// <summary>
        /// Gets/sets post string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Post> Post { get; private set; }

        /// <summary>
        /// Gets/sets site string localization.
        /// </summary>
        public IStringLocalizer<Aero.Cms.Manager.Localization.Site> Site { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ManagerLocalizer(
            IStringLocalizer<Aero.Cms.Manager.Localization.Alias> alias,
            IStringLocalizer<Aero.Cms.Manager.Localization.Comment> comment,
            IStringLocalizer<Aero.Cms.Manager.Localization.Content> content,
            IStringLocalizer<Aero.Cms.Manager.Localization.Config> config,
            IStringLocalizer<Aero.Cms.Manager.Localization.General> general,
            IStringLocalizer<Aero.Cms.Manager.Localization.Security> security,
            IStringLocalizer<Aero.Cms.Manager.Localization.Language> language,
            IStringLocalizer<Aero.Cms.Manager.Localization.Media> media,
            IStringLocalizer<Aero.Cms.Manager.Localization.Menu> menu,
            IStringLocalizer<Aero.Cms.Manager.Localization.Module> module,
            IStringLocalizer<Aero.Cms.Manager.Localization.Page> page,
            IStringLocalizer<Aero.Cms.Manager.Localization.Post> post,
            IStringLocalizer<Aero.Cms.Manager.Localization.Site> site)
        {
            Alias = alias;
            Comment = comment;
            Content = content;
            Config = config;
            General = general;
            Security = security;
            Language = language;
            Media = media;
            Menu = menu;
            Module = module;
            Page = page;
            Post = post;
            Site = site;
        }
    }
}
