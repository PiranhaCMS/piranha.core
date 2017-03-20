### Automatically adjust textarea height based on user input.
#### (Non-sucking version)



The only reason I wrote this jQuery plugin is that other plugins suck. They are wide-spread, but outdated and buggy.

Advantages of jquery.ns-autogrow:

* Grows vertically, horizontally or both
* Correctly handles 2 or more spaces
* Copies more css and font styles to shadow div
* Correctly handles long words on one line
* Flickering can be disabled on Enter
* Doesn't add more than one handler to textarea
* Handles textarea scrollbar if any
* Improved support of special characters

## Download
Run one of these commands in your bash according to your needs.

`git clone https://github.com/ro31337/jquery.ns-autogrow.git`

`bower install jquery.ns-autogrow`

`npm install jquery.ns-autogrow`

Or download the latest version from the [releases](https://github.com/ro31337/jquery.ns-autogrow/releases) page.

### Options

You can provide multiple options to autogrow like:

```
  $('.example2 textarea').autogrow({vertical: true, horizontal: false});
```

List of options:

Option | Description
-------|------------
vertical | (true/false) - Enable/Disable vertical autogrow (true by default)
horizontal | (true/false) - Enable/Disable horizontal autogrow (true by default)
postGrowCallback | Post grow callback. Executes after dimensions of textarea have been adjusted.
flickering | (true/false) - Enable/Disable flickering. If flickering is disabled, extra line will be added to textarea. Flickering is _enabled_ by default.

There are few more options reserved for debugging purposes. All debugging options start with `debug` prefix:

Option | Description
-------|------------
debugx | X position of shadow element (-10000 by default)
debugy | Y position of shadow element (-10000 by default)
debugcolor | Color of shadow element (yellow by default)


### Demo

[Click here](http://htmlpreview.github.io/?https://raw.githubusercontent.com/ro31337/jquery.ns-autogrow/master/demo/index.html)

### Plans:

* Test and support arabic languages

### :heart: Like it? :heart:

:star: Star it! :star:

### The MIT License (MIT)

The MIT License (MIT)

Copyright (c) 2015 Roman Pushkin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
