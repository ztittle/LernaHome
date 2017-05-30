using System;
using System.Collections.Generic;
using VDS.RDF;

namespace LernaHome.DotNetRdf
{
    public class FluentGraphBuilder
    {
        private IGraph _graph;
        private INode _subject;
        private ICollection<IUriNode> predicates = new List<IUriNode>();

        public FluentGraphBuilder(INode subject)
        {
            _graph = subject.Graph;
            _subject = subject;
        }

        public FluentGraphPredicateBuilder WithPredicate(string qname)
        {
            var pob = new FluentGraphPredicateBuilder(_subject, _graph.CreateUriNode(qname));
            return pob;
        }
    }

    public class FluentGraphPredicateBuilder
    {
        private IGraph _graph;
        private INode _subjectNode;
        private IUriNode _predicateNode;
        private ICollection<INode> _objectNodes = new List<INode>();

        public FluentGraphPredicateBuilder(INode subject, IUriNode predicateNode)
        {
            _graph = subject.Graph;
            _subjectNode = subject;
            _predicateNode = predicateNode;
        }

        public FluentGraphPredicateBuilder AndAnonymousObject(out IBlankNode bnode)
        {
            bnode = _graph.CreateBlankNode();
            _objectNodes.Add(bnode);
            return this;
        }

        public FluentGraphPredicateBuilder AndLiteralObject(string literal)
        {
            _objectNodes.Add(_graph.CreateLiteralNode(literal));
            return this;
        }


        public FluentGraphPredicateBuilder AndLiteralObject(string literal, string dataTypeQname)
        {
            _objectNodes.Add(_graph.CreateLiteralNode(literal, _graph.ResolveQName(dataTypeQname)));
            return this;
        }

        public FluentGraphPredicateBuilder AndNamedObject(string qname)
        {
            _objectNodes.Add(_graph.CreateUriNode(qname));
            return this;
        }

        public FluentGraphPredicateBuilder AndNamedObject(Uri iri)
        {
            _objectNodes.Add(_graph.CreateUriNode(iri));
            return this;
        }

        public IGraph Assert()
        {
            foreach(var objNode in _objectNodes)
            {
                _graph.Assert(_subjectNode, _predicateNode, objNode);
            }

            return _graph;
        }
    }
}
