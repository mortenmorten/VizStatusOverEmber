namespace VizStatusOverEmberLib
{
    using System.Text.RegularExpressions;

    public class Command
    {
        public Command(string input)
        {
            var matches = Regex.Matches(input, @"([-]\d+)?(\w*)");
            var categoryIdx = 0;

            if (int.TryParse(GetValue(matches, 0, "-1"), out var id))
            {
                Id = id;
                categoryIdx = 2;
            }
            else
            {
                Id = -1;
            }

            Category = GetValue(matches, categoryIdx, string.Empty);
            categoryIdx += 2;
            Name = GetValue(matches, categoryIdx, string.Empty);
            categoryIdx += 2;
            Value = GetValue(matches, categoryIdx, string.Empty);
        }

        public int Id { get; }

        public string Category { get; }

        public string Name { get; }

        public string Value { get; }

        public void ThrowIfDefaultValues()
        {
            if (string.IsNullOrEmpty(Category))
            {
                throw new CommandException("Missing Category");
            }

            if (string.IsNullOrEmpty(Name))
            {
                throw new CommandException("Missing Name");
            }

            if (string.IsNullOrEmpty(Value))
            {
                throw new CommandException("Missing Value");
            }
        }

        public override string ToString()
        {
            return $"{Id} {Category} {Name} {Value}".Trim();
        }

        private static string GetValue(MatchCollection matches, int match, string defaultValue)
        {
            return matches == null || matches.Count <= match || !matches[match].Success
                ? defaultValue
                : matches[match].Value;
        }
    }
}