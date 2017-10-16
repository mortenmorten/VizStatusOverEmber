namespace VizStatusOverEmberLib.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestCommand
    {
        [Test]
        public void Constructor_ValidInputStringWithoutId_PropertiesContainsParsedValues()
        {
            var cmd = new Command("input 1 active");
            Assert.That(cmd.Id, Is.EqualTo(-1));
            Assert.That(cmd.Category, Is.EqualTo("input"));
            Assert.That(cmd.Name, Is.EqualTo("1"));
            Assert.That(cmd.Value, Is.EqualTo("active"));
        }

        [Test]
        public void Constructor_ValidInputStringWithId_PropertiesContainsParsedValues()
        {
            var cmd = new Command("12 input 1 active");
            Assert.That(cmd.Id, Is.EqualTo(12));
            Assert.That(cmd.Category, Is.EqualTo("input"));
            Assert.That(cmd.Name, Is.EqualTo("1"));
            Assert.That(cmd.Value, Is.EqualTo("active"));
        }

        [Test]
        public void Constructor_MissingCategory_ThrowsException()
        {
            var cmd = new Command("");
            Assert.Throws<CommandException>(cmd.ThrowIfDefaultValues);
        }

        [Test]
        public void Constructor_MissingName_ThrowsException()
        {
            var cmd = new Command("input");
            Assert.Throws<CommandException>(cmd.ThrowIfDefaultValues);
        }

        [Test]
        public void Constructor_MissingValue_ThrowsException()
        {
            var cmd = new Command("input 1");
            Assert.Throws<CommandException>(cmd.ThrowIfDefaultValues);
        }
    }
}
