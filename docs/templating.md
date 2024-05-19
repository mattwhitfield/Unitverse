# Templating 📐

**This is pre-release information - templates are new, not finished and could well change**

Templating in Unitverse allows you to specify your own templates which will be used as the basis of tests, meaning that you can emit tests for common scenarios that exist in your problem domain but don't necessarily make sense to be added to the tool.

Templates are loaded from `*.template` files that are stored in a folder called `.unitverseTemplates`. These templates can be stored anywhere that is a parent path of the project for which tests are being generated (in the same way that `.unitTestGeneratorConfig` and `.editorConfig` files are found).

Templates can be targeted against properties, methods and constructors.

Templates use DotLiquid markup, so you can do anything that you can do in DotLiquid.

## File format 💾

Templates are a set of headers, a blank line and then the template content.

### Headers

The headers of the template are as follows:

| Header | Required | Meaning |
| - | - | - |
| TestMethodName | Yes | This is the name for the test method that is generated. It uses the same syntax as the naming options. |
| Target | Yes | This is the type of member that the template is targeting - can be 'Method', 'Property' or 'Constructor' |
| Include | Yes | This is the filter expression that must return true for the template to be used for any particular member. You can define more than one Include header - if you do then all must return true for the template to be selected. |
| Exclude | No | This is a filter expression that is used to stop a template being used. You can define zero or more Exclude headers and if any of them return true then the template is not selected. |
| IsAsync | No | If you specify `True` for this option, then the emitted method is `async` |
| IsStatic | No | If you specify `True` for this option, then the emitted method is `static` |
| Description | No | This is a textual description for the template. If you emit XML comments for your tests, the description will be included there. |
| IsExclusive | No | If you specify `True` for this option, then the template can only be used if no prior templates have been matched to the current member. |
| StopMatching | No | If you specify `True` for this option, then matching of templates stops after this rule is applied. |
| Priority | No | Specifying the priority of a template allows for their ordering. 1 is a higher priority than 10. The default priority (for when this header is not specified) is 10. |

## Selectivity 🏅

You can define multiple `Include` and `Exclude` expressions for templates. At least one `Include` is required - and all `Include` expressions must match for the template to be used. If any `Exclude` expressions are matched then the template will not be used.

The expressions use the [SequelFilter grammar](https://mattwhitfield.github.io/SequelFilter/grammar.html) in order to match templates against the members for which they should generate. There are two root members defined for you to filter on - `model` which is the filter model type for the member and `owningType` which is the filter model type for the type to which the member belongs. Depending on which member type your template targets, the `model` will be different (as in the table below).

### Models

Templates get different types in their context depending on what they target. For each there is 'Model' which is the member being generated for, and 'OwningType' which is the type to which the member belongs.

| Target | Model | OwningType |
| - | - | - |
| Property | [IProperty](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IProperty.cs) | [IOwningType](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IOwningType.cs) |
| Method | [IMethod](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IMethod.cs) | [IOwningType](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IOwningType.cs) |
| Constructor | [IConstructor](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IConstructor.cs) | [IOwningType](https://github.com/mattwhitfield/Unitverse/blob/master/src/Unitverse.Core/Templating/Model/IOwningType.cs) |

### Example filter expressions

| Expression | Meaning |
| - | - |
| Model.Type.Name == 'Int32' | The model's type is 'Int32' |
| OwningType.Methods HAS_ANY x => x.Name == 'Serialize' | The model's owning type has a method called 'Serialize' |
| Model.Attibutes HAS_ANY x => x.Type.Name == 'HttpPostAttribute' | The model has an attribute `[HttpPost]` |

## Template debugging 🕷

Hold down Ctrl+Alt while clicking 'Generate tests' and the filter expression tester will appear. This will show you the model that is generated for each selected member, and allow you to write and test templates against it to see which filter expressions match and how the template is resolved.

TODO - actually implement this functionality 🤣

## Example template 👀

Here is an example template that will be generated for all properties of type `int`:

<!-- {% raw %} -->
```
TestMethodName: {memberName:pascal}IsGreaterThan5AndLessThan100
Target: Property
Priority: 1
Include: Model.Type.Name == 'Int32'

// Verify that {{model.Name}} is between 5 and 100
Assert.That(_testClass.{{model.Name}}, Is.GreaterThan(5));
Assert.That(_testClass.{{model.Name}}, Is.LessThan(100));
```
<!-- {% endraw %} -->

The output generated is:

```
[Fact]
public void TheNumberIsGreaterThan5AndLessThan100()
{
    // Verify that TheNumber is between 5 and 100
    Assert.That(_testClass.TheNumber, Is.GreaterThan(5));
    Assert.That(_testClass.TheNumber, Is.LessThan(100));
}
```
