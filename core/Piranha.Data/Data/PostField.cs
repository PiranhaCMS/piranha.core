/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Post fields hold the actual content of the posts.
	/// </summary>
	public sealed class PostField : Base.ContentField<Post, PostType, PostTypeField> { }
}
