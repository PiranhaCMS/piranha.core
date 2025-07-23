/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.LocalAuth;

/// <summary>
/// The different results a login can have.
/// </summary>
public enum LoginResult
{
    /// <summary>
    /// The login succeeded.
    /// </summary>
    Succeeded,
    /// <summary>
    /// The login failed.
    /// </summary>
    Failed,
    /// <summary>
    /// The user account is locked.
    /// </summary>
    Locked
}