namespace Unitverse.Core.Options
{
    using System.Linq;
    using SequelFilter.Resolvers;

    public class ObjectPathResolver : TokenResolver<IFieldReferenceResolver>
    {
        public ObjectPathResolver(string pattern)
            : base(pattern)
        {
        }

        protected override bool GetTokenValue(IFieldReferenceResolver context, string token, out string? value)
        {
            var target = context.Resolve(token.Split('.').Select(x => x.Trim()).ToList(), 0);
            if (target != null)
            {
                value = target.ToString();
                return true;
            }

            value = default;
            return false;
        }
    }
}
