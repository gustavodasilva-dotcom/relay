namespace Relay.SharedKernel.UnitTests;

public sealed class ValueObjectTests
{
    [Fact]
    public void Equal_operator_returns_true_when_atomic_values_are_equal()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new TestValueObject(1);

        // Act & Assert
        Assert.True(left == right);
    }

    [Fact]
    public void Equal_operator_returns_false_when_atomic_values_are_different()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new TestValueObject(2);

        // Act & Assert
        Assert.False(left == right);
    }

    [Fact]
    public void Equal_operator_returns_false_for_different_value_object_types()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new OtherTestValueObject(1);

        // Act & Assert
        Assert.False(left == right);
    }

    [Fact]
    public void Not_equal_operator_returns_true_when_atomic_values_are_different()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new TestValueObject(2);

        // Act & Assert
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_returns_true_when_atomic_values_are_equal()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new TestValueObject(1);

        // Act & Assert
        Assert.True(left.Equals(right));
        Assert.True(left.Equals((object)right));
    }

    [Fact]
    public void Equal_value_objects_have_the_same_hash_code()
    {
        // Arrange
        var left = new TestValueObject(1);
        var right = new TestValueObject(1);

        // Act & Assert
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equal_operator_returns_true_when_both_values_are_null()
    {
        // Arrange
        TestValueObject? left = null;
        TestValueObject? right = null;

        // Act & Assert
        Assert.True(left == right);
    }

    [Fact]
    public void Equal_operator_returns_false_when_only_one_value_is_null()
    {
        // Arrange
        TestValueObject? left = null;
        var right = new TestValueObject(1);

        // Act & Assert
        Assert.False(left == right);
        Assert.True(left != right);
    }

    private sealed class TestValueObject(int value) : ValueObject
    {
        public int Value { get; } = value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }

    private sealed class OtherTestValueObject(int value) : ValueObject
    {
        public int Value { get; } = value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
