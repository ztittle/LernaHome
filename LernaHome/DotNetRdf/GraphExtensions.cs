using System;
using LernaHome.DotNetRdf;

namespace VDS.RDF
{
    public static class GraphExtensions
    {
        public static IGraph Assert(this IGraph graph, string subjectQname, string predicateQname, string objectQname)
        {
            graph.Assert(graph.CreateUriNode(subjectQname), graph.CreateUriNode(predicateQname), graph.CreateUriNode(objectQname));
            return graph;
        }

        public static IGraph Assert(this IGraph graph, string subjectQname, string predicateQname, ILiteralNode literalNode)
        {
            graph.Assert(graph.CreateUriNode(subjectQname), graph.CreateUriNode(predicateQname), literalNode);
            return graph;
        }

        public static FluentGraphBuilder AddSubject(this IGraph graph, IBlankNode bnode)
        {
            return new FluentGraphBuilder(bnode);
        }

        public static FluentGraphBuilder AddSubject(this IGraph graph, Uri uri)
        {
            return new FluentGraphBuilder(graph.CreateUriNode(uri));
        }

        public static FluentGraphBuilder AddSubject(this IGraph graph, string qname)
        {
            return new FluentGraphBuilder(graph.CreateUriNode(qname));
        }

        public static FluentGraphBuilder AddAnonymousSubject(this IGraph graph, out IBlankNode bnode)
        {
            bnode = graph.CreateBlankNode();
            return new FluentGraphBuilder(bnode);
        }

        public static IGraph WithDefaultNamespaces(this IGraph graph)
        {
            graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            graph.NamespaceMap.AddNamespace("xsd", new Uri("http://www.w3.org/2001/XMLSchema#"));
            graph.NamespaceMap.AddNamespace("hydra", new Uri("http://www.w3.org/ns/hydra/core#"));
            graph.NamespaceMap.AddNamespace("zwave", new Uri("http://example.com/zwave/"));

            return graph;
        }
    }
}
