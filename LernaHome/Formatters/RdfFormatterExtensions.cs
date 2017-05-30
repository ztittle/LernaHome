using ZWaveAdmin.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcRdfMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddRdfFormatters(this IMvcCoreBuilder builder)
        {
            builder.AddMvcOptions(opt => opt.OutputFormatters.Add(new RdfTurtleOutputFormatter()));
            builder.AddMvcOptions(opt => opt.OutputFormatters.Add(new RdfNTriplesOutputFormatter()));
            builder.AddMvcOptions(opt => opt.OutputFormatters.Add(new JsonLdOutputFormatter()));

            builder.AddMvcOptions(opt => opt.InputFormatters.Add(new RdfTurtleInputFormatter()));
            return builder;
        }
    }
}
