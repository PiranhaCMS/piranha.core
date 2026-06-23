# Security Policy

## This Fork

This is the **MinooTrading SPC** security-hardened fork of [PiranhaCMS/piranha.core](https://github.com/PiranhaCMS/piranha.core), maintained by [Kiarash Minoo](https://github.com/KiarashMinoo).

## Supported Versions

| Version       | Supported          | Notes |
| ------------- | ------------------ | ----- |
| 4.x – 9.x    | :x:                | Upstream EOL |
| 10.x upstream | :x:                | Upstream only — not supported in this fork |
| 1.0.x (fork)  | :white_check_mark: | This fork — security patches applied on top of upstream 10.x |

## Security Fixes in This Fork (v1.0.1)

The following CodeQL vulnerabilities from the upstream 10.x codebase have been resolved:

| Rule | File | Fix |
|------|------|-----|
| `cs/web/cookie-secure-not-set` | `AuthController.cs` | Added `Secure = true` + `SameSite = Strict` to XSRF cookie |
| `cs/web/missing-token-validation` | `CmsController.cs` | Added `[ValidateAntiForgeryToken]` to `SavePostComment` |
| `cs/user-controlled-bypass` | `ModelLoader.cs` | Hardened `draft` flag checks with explicit authorization gates |
| `js/xss-through-dom` | Manager JS bundles | DOM XSS in Bootstrap Carousel and Tooltip/Popover patched |
| `js/incomplete-sanitization` | Manager JS bundles | Added `g` flag to marked.js table-sanitization regex |
| `js/unsafe-html-expansion` | Manager JS bundles | jQuery `htmlPrefilter` no longer mutates self-closing tags |

## Reporting a Vulnerability

To report a vulnerability in **this fork**, open a private security advisory at:
https://github.com/MinooTradingSPC/piranha.core/security/advisories/new

Or contact the maintainer directly: **ahmed.m@aau.iq**

To report a vulnerability in the **upstream project**, contact the PiranhaCMS team at info@piranhacms.org.
