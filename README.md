# Void
## A fearless modern text editor in the spirit of Vim
[![Mono Build Status](https://travis-ci.org/PolyglotSymposium/void.svg?branch=master)](https://travis-ci.org/PolyglotSymposium/void)
[![.NET Build Status](https://ci.appveyor.com/api/projects/status/whn1xp1y06viy6t8/branch/master?svg=true)](https://ci.appveyor.com/project/Kazark/void/branch/master)

## Nota Bene
This project is still in early development!

### What?
A text editor that is not afraid to break compatibility with Vim, but is
fundamentally inspired by it and dedicated to the spirit of Vim.

### Why?
Vim is a great text editor. To my mind, it is _the_ great text editor. Even if
Void goes much farther than I anticipate, it will never reach the greatness of
Vim.

That being said, there are things about Vim that really bother me. I say that
not as a hater, but as someone who has deeply dedicated himself to using Vim for
everything.

[NeoVim](https://github.com/neovim/neovim) is an exciting project. I truly hope
and believe that it is the Vim of the future. However, while it is dropping a
lot of crufty old Vi compatibility, it is still trying to be _Vim_ (for the
twenty-first century). I want to build something that loves on Vim without being
afraid to break compatibility with it.

Almost any significant editor or IDE has a Vi or Vim mode. However, at the
points that the editor diverges from Vi or Vim, it does so for the underlying
conceptual model of that editor, which is different than that of Vim. I want to
create something that diverges from Vim only because it loves Vim, something
that when it diverges from Vim, still feels Vim-like.

### What's with the name?
_Void_ is derived from "Vim-oid", that is, it is a Vim-like text editor.

### Goals
+ Honor the inherent conflict of interest between command-mode and a scripting
  language. Vi is fundamentally modal; I believe that there needs to be a modal
  distinction drawn between wanting to dash off quick commands or while working
  or even do some simple configuration, and doing some serious scripting such as
  implementing a whole plugin.
+ Bind keys to the underlying API rather than mapping them to other keys.
+ Default some keys to more useful default mappings, as many Vim users are
  already doing (<kbd>CTRL</kbd>-<kbd>a</kbd> and <kbd>CTRL</kbd>-<kbd>c</kbd>
  and possibly <kbd>CTRL</kbd>-<kbd>v</kbd> should follow CUA standards; use `;`
  rather than `:` to enter command mode; <kbd>TAB</kbd> and
  <kbd>SHIFT</kbd>-<kbd>TAB</kbd> should indent and de-indent...)
+ Integrate brilliant ideas from plugins that really should be canonical into
  the core, particularly many of those by the great @tpope such as
  [Surround](https://github.com/tpope/vim-surround) and
  [Fugitive](https://github.com/tpope/vim-fugitive) (a Git wrapper so awesome,
  it should be ~~illegal~~ in everyone's text editor). One of the things that
  makes Tim Pope great is that his plugins make you think, "This feels so native
  and good, I can't believe it's not part of the core Vim."
+ Run my shell sessions within my editor, instead of my editor within my shell,
  so I can use all of Vim's slick Window and Tab navigation commands seamlessly
  rather than having to deal with two sets of tabs and two sets of commands (or
  mouse clicks) such as what happens when Vim is hosted in another editor
+ Make use of GUI to increase usability
+ Improve searchability of documentation
+ Reduce number of options/center configurability more solidly around filetypes

### Contributing
I would love to see pull requests, but I recommend that if you are implementing
a new feature you submit an issue first so we can discuss the new feature, even
if the "new" feature is a Vim feature, because I do not intend to
indiscriminately accept all Vim features into Void. That being said, please
contribute!

## Disclaimer
And yes, I know...

![One does not simply reimplement Vim](http://i.imgur.com/cNmpx3x.jpg)
