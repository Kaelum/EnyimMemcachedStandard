# Enyim Memcached Client

This is a .NET Standard 2.0 port of [EnyimMemcached](https://github.com/enyim/EnyimMemcached).

Features:

* Understands both the binary and text protocols
* Highly configurable and extendable (custom configuration, serialization)
* Supports consistent hashing
* CheckAndSet operations
* Persistent connections for more speed
* SASL Authentication

## Requirements

You'll need .NET Standard 2.0 or later to use the precompiled binaries. To build client, you'll need Visual Studio 2017.

## Bugs and features

If you found any issues with the client, please submit a bug report at the [Issue Tracker](http://github.com/Kaelum/EnyimMemcachedStandard/issues). Please include the following:

- The version of the client you are using
- The relevant configuration section from your app/web.config
- How to reproduce the issue (either a step-by-step description or a small test application)
- Expected result

If you'd like to add a feature request please add some details how it is supposed to work.

## Patches

The best bug reports come with a patch: fork the code on GitHub, then send a pull request so the fixes can be included in the next version.