This repository is forked from https://github.com/sentryone/unittestgenerator.

The license of the original repository is Apache 2.0. From 30th Nov 2021, the license is switching to MIT. To comply with the license requirements of Apache 2.0, here are the changes that have been made from the original repository to 30th Nov 2021 (from https://github.com/mattwhitfield/unittestgenerator/commit/14d78a0004fa4dbaf7a6394fa3706eb6eb2ae936 until https://github.com/mattwhitfield/unittestgenerator/commit/09acd3acc80f470ab10d325384e0dec475e6fb24):

* Updates for build purposes
* Removal of automatic project generation
* Removal of support for Rhino Mocks
* Update to partial generation to yield consistent test method names when overloaded methods are present
* Better support for emitting tests when @reserved words were used as identifiers 
* Support for FluentAssertions
* Better support for Moq (field type and mock type differentiation)
* Auto-detection of target frameworks
* Smarter type aliases if T is constrained
* Support for nullable reference types
* Support for customisable naming conventions
* Support for Visual Studio 2022
* Rebrand to Unitverse
* Addition of GitHub Actions pipeline

Any further project updates will not be noted here.