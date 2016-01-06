# HEPOntologyParserExperiments
Experiments with a DSL that will parse to a FinalStateObject OWL

# DSL

The DSL is simple. For example, this specifies an exotics search for displaced jets (http://arxiv.org/abs/1501.04020).

```
   J1 (anti-kt-0.4): et>60 GeV, abseta < 2.5, EMF < 0.02, -1 ns < ArrivalTime < 5 ns;
   J2 (anti-kt-0.4): et>40 GeV, abseta < 2.5, EMF < 0.02, -1 ns < ArrivalTime < 5 ns;
   Veto: J3 (anti-kt-0.4): et>40 GeV, abseta < 2.5, EMF < 0.02, -1 ns < ArrivalTime < 5 ns;
   PV1 (primary-vertex): NTrack >= 3, pT>1 GeV;
   NTrack(J1, DR=0.2, pT>1) = 0;
   NTrack(J2, DR=0.2, pT>1) = 0;
   ETMiss (atlas-ref) < 50 GeV;
```

#The current state

It will parse the below definition just fine. Two obvious things are missing:

   - Using a "." in a base definition name.
   - "Veto:" is not yet implemented.

Both of these shouldn't be hard to add. There are a number of issues in the code:

   - Look for TODO to see notes about things that are poorly implemented or otherwise. This is not production code!!
   - We need a more general expression to parse things in ANTLR4, the current scheme is rather messy and
     involves a really ugly set of parsing interpreting code. Someone who knows how better to use g4 will quickly
	 recognize my mistakes, and perhaps, solutions. :-)
   - There are places where this DSL is ambiguous, and only if the lexer has some context awareness can the ambiguities be
     removed.
   - Some thought has to go into how the functions are represented in OWL, perhaps for general searching they are
     currently general enough.
   - ETMiss is an object, and a quantity (e.g. ETMiss.ET is the way you shoudl write it, but everyone will want to write ETMiss).
     We need some sort of common abbreviations to handle that.
   - Nuget has an interesting RDFSharp library that seems to have classes for dealing with ontologies. It would be cool to fit this in there.
     In particular because it seems to have normalized i/o routines

Here is the DSL it does parse:

```
   J1 (anti-kt-04): et>60 GeV, abseta < 2.5, EMF < 0.02, -1 ns < ArrivalTime < 5 ns;
   J2 (anti-kt-04): et>40 GeV, abseta < 2.5, EMF < 0.02, -1 ns < ArrivalTime < 5 ns;
   PV1 (primary-vertex): NTrack >= 3, pT>1 GeV;
   NTrack(J1, DR=0.2, pT>1) = 0;
   NTrack(J2, DR=0.2, pT>1) = 0;
   ETMiss (atlas-ref) < 50 GeV;
```

The OWL dump for this looks like the following:

```
<#J1> rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "anti-kt-04" .

<#J2> rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "anti-kt-04" .

<#PV1> rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "primary-vertex" .

<#ETMiss> rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "atlas-ref" .

<#physicalQuantity0> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> ;
  dfs:hasQuantity dfs:et .

<#number1> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "60"^^xsd:decimal ;
  dfs:hasUnit dfs:GeV .

<#selectionCriteria2> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument <#physicalQuantity0> ;
  dfs:hasSecondArgument <#number1> .

<#physicalQuantity3> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> ;
  dfs:hasQuantity dfs:abseta .

<#number4> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "2.5"^^xsd:decimal .

<#selectionCriteria5> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity3> ;
  dfs:hasSecondArgument <#number4> .

<#physicalQuantity6> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> ;
  dfs:hasQuantity dfs:EMF .

<#number7> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "0.02"^^xsd:decimal .

<#selectionCriteria8> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity6> ;
  dfs:hasSecondArgument <#number7> .

<#number9> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "-1"^^xsd:decimal ;
  dfs:hasUnit dfs:ns .

<#physicalQuantity10> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> ;
  dfs:hasQuantity dfs:ArrivalTime .

<#selectionCriteria11> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#number9> ;
  dfs:hasSecondArgument <#physicalQuantity10> .

<#physicalQuantity12> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> ;
  dfs:hasQuantity dfs:ArrivalTime .

<#number13> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "5"^^xsd:decimal ;
  dfs:hasUnit dfs:ns .

<#selectionCriteria14> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity12> ;
  dfs:hasSecondArgument <#number13> .

<#physicalQuantity15> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> ;
  dfs:hasQuantity dfs:et .

<#number16> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "40"^^xsd:decimal ;
  dfs:hasUnit dfs:GeV .

<#selectionCriteria17> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument <#physicalQuantity15> ;
  dfs:hasSecondArgument <#number16> .

<#physicalQuantity18> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> ;
  dfs:hasQuantity dfs:abseta .

<#number19> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "2.5"^^xsd:decimal .

<#selectionCriteria20> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity18> ;
  dfs:hasSecondArgument <#number19> .

<#physicalQuantity21> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> ;
  dfs:hasQuantity dfs:EMF .

<#number22> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "0.02"^^xsd:decimal .

<#selectionCriteria23> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity21> ;
  dfs:hasSecondArgument <#number22> .

<#number24> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "-1"^^xsd:decimal ;
  dfs:hasUnit dfs:ns .

<#physicalQuantity25> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> ;
  dfs:hasQuantity dfs:ArrivalTime .

<#selectionCriteria26> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#number24> ;
  dfs:hasSecondArgument <#physicalQuantity25> .

<#physicalQuantity27> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> ;
  dfs:hasQuantity dfs:ArrivalTime .

<#number28> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "5"^^xsd:decimal ;
  dfs:hasUnit dfs:ns .

<#selectionCriteria29> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity27> ;
  dfs:hasSecondArgument <#number28> .

<#physicalQuantity30> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#PV1> ;
  dfs:hasQuantity dfs:NTrack .

<#number31> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "3"^^xsd:decimal .

<#selectionCriteria32> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterEqual ;
  dfs:hasFirstArgument <#physicalQuantity30> ;
  dfs:hasSecondArgument <#number31> .

<#physicalQuantity33> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#PV1> ;
  dfs:hasQuantity dfs:pT .

<#number34> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "1"^^xsd:decimal ;
  dfs:hasUnit dfs:GeV .

<#selectionCriteria35> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument <#physicalQuantity33> ;
  dfs:hasSecondArgument <#number34> .

<#functionQuantity36> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J1> , <#FuncArgNTrack> ;
  dfs:hasQuantity "NTrack(J1, DR=0.2, pT>1)" .

<#number37> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "0"^^xsd:decimal .

<#selectionCriteria38> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:equal ;
  dfs:hasFirstArgument <#functionQuantity36> ;
  dfs:hasSecondArgument <#number37> .

<#functionQuantity39> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#J2> , <#FuncArgNTrack> ;
  dfs:hasQuantity "NTrack(J2, DR=0.2, pT>1)" .

<#number40> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "0"^^xsd:decimal .

<#selectionCriteria41> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:equal ;
  dfs:hasFirstArgument <#functionQuantity39> ;
  dfs:hasSecondArgument <#number40> .

<#physicalQuantity42> rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject <#ETMiss> ;
  dfs:hasQuantity dfs: .

<#number43> rdf:type dfs:NumericalValue ;
  dfs:hasNumber "50"^^xsd:decimal ;
  dfs:hasUnit dfs:GeV .

<#selectionCriteria44> rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument <#physicalQuantity42> ;
  dfs:hasSecondArgument <#number43> .

<#andor45> rdf:type dfs:And ;
  dfs:hasOperand <#selectionCriteria2> , <#selectionCriteria5> , <#selectionCriteria8> , <#selectionCriteria11> , <#selectionCriteria14> , <#selection
Criteria17> , <#selectionCriteria20> , <#selectionCriteria23> , <#selectionCriteria26> , <#selectionCriteria29> , <#selectionCriteria32> , <#selection
Criteria35> , <#selectionCriteria38> , <#selectionCriteria41> , <#selectionCriteria44> .

<#detectorFinalState46> rdf:type dfs:DetectorFinalState ;
  dfs:hasSelectionCriteria <#andor45> .

```

# Development

The environment for devleopment is easy to setup:

1. Use Visual Stuido 2015 Community Edition (Free). It may work with older versions, like 2013, but primary development isn't done there.

2. Install the Java Runtime. No need for browser integration. This is needed for the ANTLR code, which uses java to produce the parser.
