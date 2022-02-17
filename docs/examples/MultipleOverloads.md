﻿# Multiple Overloads
Shows how unitverse generates unambiguous names for methods that test multiple overloads of the same source method

### Source Type(s)
``` csharp
public static class FluentFactory
{
    public static Tuple<Stage, IList<Stage>> Follows(this Stage stage, params Stage[] followedStages)
    {
        return null;
    }

    public static IDictionary<Stage, IList<Stage>> And(this Tuple<Stage, IList<Stage>> firstConstraint, Tuple<Stage, IList<Stage>> secondConstraint)
    {
        return null;
    }

    public static IDictionary<Stage, IList<Stage>> And(this IDictionary<Stage, IList<Stage>> constraints, Tuple<Stage, IList<Stage>> additionalConstraint)
    {
        return null;
    }
}

```

### Generated Tests
``` csharp
public static class FluentFactoryTests
{
    [Fact]
    public static void CanCallFollows()
    {
        // Arrange
        var stage = Stage.First;
        var followedStages = new[] { Stage.First, Stage.Second, Stage.Fourth };

        // Act
        var result = stage.Follows(followedStages);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallFollowsWithNullFollowedStages()
    {
        FluentActions.Invoking(() => Stage.Third.Follows(default(Stage[]))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallAndWithFirstConstraintAndSecondConstraint()
    {
        // Arrange
        var firstConstraint = new Tuple<Stage, IList<Stage>>(Stage.Second, new[] { Stage.Second, Stage.Fourth, Stage.First });
        var secondConstraint = new Tuple<Stage, IList<Stage>>(Stage.Third, new[] { Stage.First, Stage.First, Stage.Second });

        // Act
        var result = firstConstraint.And(secondConstraint);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallAndWithFirstConstraintAndSecondConstraintWithNullFirstConstraint()
    {
        FluentActions.Invoking(() => default(Tuple<Stage, IList<Stage>>).And(new Tuple<Stage, IList<Stage>>(Stage.Fourth, new[] { Stage.Third, Stage.Third, Stage.Second }))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CannotCallAndWithFirstConstraintAndSecondConstraintWithNullSecondConstraint()
    {
        FluentActions.Invoking(() => new Tuple<Stage, IList<Stage>>(Stage.Third, new[] { Stage.Third, Stage.Third, Stage.Fourth }).And(default(Tuple<Stage, IList<Stage>>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallAndWithConstraintsAndAdditionalConstraint()
    {
        // Arrange
        var constraints = Substitute.For<IDictionary<Stage, IList<Stage>>>();
        var additionalConstraint = new Tuple<Stage, IList<Stage>>(Stage.First, new[] { Stage.First, Stage.Second, Stage.Fourth });

        // Act
        var result = constraints.And(additionalConstraint);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallAndWithConstraintsAndAdditionalConstraintWithNullConstraints()
    {
        FluentActions.Invoking(() => default(IDictionary<Stage, IList<Stage>>).And(new Tuple<Stage, IList<Stage>>(Stage.First, new[] { Stage.Fourth, Stage.Second, Stage.Fourth }))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CannotCallAndWithConstraintsAndAdditionalConstraintWithNullAdditionalConstraint()
    {
        FluentActions.Invoking(() => Substitute.For<IDictionary<Stage, IList<Stage>>>().And(default(Tuple<Stage, IList<Stage>>))).Should().Throw<ArgumentNullException>();
    }
}

```
