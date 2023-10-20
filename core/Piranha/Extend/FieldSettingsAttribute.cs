/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend;

/// <summary>
/// Base class for all field settings attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class FieldSettingsAttribute : Attribute { }
