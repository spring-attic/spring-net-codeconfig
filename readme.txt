The Spring Code Config project for .NET, Release 1.0.0 (3/30/2011)
--------------------------------------------------------------------
https://github.com/SpringSource/spring-net-codeconfig


1. INTRODUCTION

The 1.0.0 release of Spring Code Config for NET provides:

     * the ability to configure a Spring container using standard .NET code instead of or in addition to XML configuration

2. SUPPORTED .NET FRAMEWORK VERSIONS

Spring Code Config for .NET version 1.0 supports the .NET 2.0 and later framework runtimes (but REQUIRES Visual Studio 2008 or later, even when targeting .NET 2.0 -- see KNOWN ISSUES/LIMITATIONS for details).

3. KNOWN ISSUES/LIMITATIONS

* Initial Features
This initial relase of Spring Code Config for .NET supports the basic configuration of Object Definitions Metadata related to object construction but does not (yet) support the more advanced features of the Spring.NET Dependency Injection container including the application of Aspects, the subsitution of VariablePlaceholderValues, and others.  This support will be provided in subsequent releases of this project.

* Compiler Warning  
You may receive the following warning when compiling your projects when referencing Spring Code Config for .NET: "Warning: The predefined type 'System.Runtime.CompilerServices.ExtensionAttribute' is defined in multiple assemblies in the global alias; using definition from '<path-to-System.Core.dll>'.  To extend the behavior of the AbstractApplicationContext, extension methods have been used.  To enable extension methods under .NET 2.0 where they would otherwise be unsupported, the work-around as described here http://www.danielmoth.com/Blog/Using-Extension-Methods-In-Fx-20-Projects.aspx has been employed.  Depending on your targeted runtime version, this approach can result in the preceeding warning being issued by the compiler.  This warning can be safely ignored as it will not impact the functionality of the Spring Code Config for .NET or your own application.

4. RELEASE INFO

Release contents:

* "src" contains the C# source files for the framework
* "test" contains the C# source files for the test suite
* "bin" contains the distribution dll files
* "lib/net" contains common libraries needed for building and running the framework
* "doc" contains reference documentation, MSDN-style API help, and the Spring Code Config for .NET xsd.
* "examples" contains sample applications.

The VS.NET solution for the framework and examples are provided.

Latest info is available at the public website: http://www.springframework.net/

Spring Code Config for .NET is released under the terms of the Apache Software License (see license.txt).

5. DISTRIBUTION DLLs

The "bin" directory contains the following distinct dll files for use in applications:
* Spring.Core.CodeConfig

6. WHERE TO START?

Documentation can be found in the "docs" directory:
* The Spring.NET CodeConfig reference documentation

Documented sample applications can be found in "examples":

7. How to build

VS.NET
------
The is a solution file for different version of VS.NET

* Spring.Config.2010.sln for use with VS.NET 2010

8. Support

The user forums at http://forum.springframework.net/ are available for you to submit questions, support requests, and interact with other Spring.NET users.

Bug and issue tracking can be found at https://jira.springsource.org/browse/SPRNET

To get the sources, check them out at the git repository at https://github.com/SpringSource/spring-net-codeconfig

We are always happy to receive your feedback on the forums. If you think you found a bug, have an improvement suggestion
or feature request, please submit a ticket in JIRA (see link above).

9. Acknowledgements

InnovaSys Document X!
---------------------
InnovSys has kindly provided a license to generate the SDK documentation and supporting utilities for
integration with Visual Studio.






