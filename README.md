.NET Standard Library Extension [![Build Status](https://travis-ci.org/Whathecode/NET-Standard-Library-Extension.svg?branch=master)](https://travis-ci.org/Whathecode/NET-Standard-Library-Extension)
======

This library contains highly reuseable classes and methods I find to be missing in the cross-platform standard library of .NET.

**Requires**:

- To open the solution and project files, Visual Studio 2017 is required.
- The test project requires the [.NET Core 1.1 SDK](https://www.microsoft.com/net/download/core).

Namespaces from the original library are followed as much as possible:

- Helper classes are contained within the corresponding namespace: e.g., a helper class for `System.IO.Path` will be located in `Whathecode.System.IO.PathHelper`.
- Linq extensions are located in `Whathecode.System.Linq`.
- Extension methods are placed in the relevant namespace: e.g., general extension are located in `Whathecode.System`, while extension methods when doing reflection are located in `Whathecode.System.Reflection`.
- Unit tests are placed in a Tests namespace in the corresponding namespace: e.g., `Whathecode.Tests.System.Linq`. xUnit is used. These tests also serve as examples.

Copyright (c) 2016 Steven Jeuris
The library is distributed under the terms of the MIT license (http://opensource.org/licenses/mit-license). More information can be found in "LICENSE"
