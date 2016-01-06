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
@prefix owl: <http://www.w3.org/2002/07/owl#> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix xml: <http://www.w3.org/XML/1998/namespace> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
@prefix dfs: <https://w3id.org/daspos/detectorfinalstate#> .
@prefix qudt: < http://qudt.org/1.1/schema/qudt#> .
@prefix unit: < http://qudt.org/1.1/vocab/unit#> .

atlas:J1 rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "anti-kt-04" .

atlas:J2 rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "anti-kt-04" .

atlas:PV1 rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "primary-vertex" .

atlas:ETMiss rdf:type dfs:PhysicsObject ;
  hasBaseDefinition: "atlas-ref" .

atlas:physicalQuantity0 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 ;
  dfs:refersToFinalStateObjectProperty dfs:et .

atlas:number1 rdf:type qudt:QuantityValue ;
  qudt:numericValue "60"^^xsd:decimal ;
  qudt:unit unit:GigaElectronVolt .

atlas:selectionCriteria2 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument atlas:physicalQuantity0 ;
  dfs:hasSecondArgument atlas:number1 .

atlas:physicalQuantity3 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 ;
  dfs:refersToFinalStateObjectProperty dfs:abseta .

atlas:number4 rdf:type qudt:QuantityValue ;
  qudt:numericValue "2.5"^^xsd:decimal .

atlas:selectionCriteria5 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity3 ;
  dfs:hasSecondArgument atlas:number4 .

atlas:physicalQuantity6 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 ;
  dfs:refersToFinalStateObjectProperty dfs:EMF .

atlas:number7 rdf:type qudt:QuantityValue ;
  qudt:numericValue "0.02"^^xsd:decimal .

atlas:selectionCriteria8 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity6 ;
  dfs:hasSecondArgument atlas:number7 .

atlas:number9 rdf:type qudt:QuantityValue ;
  qudt:numericValue "-1E-09"^^xsd:decimal ;
  qudt:unit unit:SecondTime .

atlas:physicalQuantity10 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 ;
  dfs:refersToFinalStateObjectProperty dfs:ArrivalTime .

atlas:selectionCriteria11 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:number9 ;
  dfs:hasSecondArgument atlas:physicalQuantity10 .

atlas:physicalQuantity12 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 ;
  dfs:refersToFinalStateObjectProperty dfs:ArrivalTime .

atlas:number13 rdf:type qudt:QuantityValue ;
  qudt:numericValue "5E-09"^^xsd:decimal ;
  qudt:unit unit:SecondTime .

atlas:selectionCriteria14 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity12 ;
  dfs:hasSecondArgument atlas:number13 .

atlas:physicalQuantity15 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 ;
  dfs:refersToFinalStateObjectProperty dfs:et .

atlas:number16 rdf:type qudt:QuantityValue ;
  qudt:numericValue "40"^^xsd:decimal ;
  qudt:unit unit:GigaElectronVolt .

atlas:selectionCriteria17 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument atlas:physicalQuantity15 ;
  dfs:hasSecondArgument atlas:number16 .

atlas:physicalQuantity18 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 ;
  dfs:refersToFinalStateObjectProperty dfs:abseta .

atlas:number19 rdf:type qudt:QuantityValue ;
  qudt:numericValue "2.5"^^xsd:decimal .

atlas:selectionCriteria20 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity18 ;
  dfs:hasSecondArgument atlas:number19 .

atlas:physicalQuantity21 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 ;
  dfs:refersToFinalStateObjectProperty dfs:EMF .

atlas:number22 rdf:type qudt:QuantityValue ;
  qudt:numericValue "0.02"^^xsd:decimal .

atlas:selectionCriteria23 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity21 ;
  dfs:hasSecondArgument atlas:number22 .

atlas:number24 rdf:type qudt:QuantityValue ;
  qudt:numericValue "-1E-09"^^xsd:decimal ;
  qudt:unit unit:SecondTime .

atlas:physicalQuantity25 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 ;
  dfs:refersToFinalStateObjectProperty dfs:ArrivalTime .

atlas:selectionCriteria26 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:number24 ;
  dfs:hasSecondArgument atlas:physicalQuantity25 .

atlas:physicalQuantity27 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 ;
  dfs:refersToFinalStateObjectProperty dfs:ArrivalTime .

atlas:number28 rdf:type qudt:QuantityValue ;
  qudt:numericValue "5E-09"^^xsd:decimal ;
  qudt:unit unit:SecondTime .

atlas:selectionCriteria29 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity27 ;
  dfs:hasSecondArgument atlas:number28 .

atlas:physicalQuantity30 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:PV1 ;
  dfs:refersToFinalStateObjectProperty dfs:NTrack .

atlas:number31 rdf:type qudt:QuantityValue ;
  qudt:numericValue "3"^^xsd:decimal .

atlas:selectionCriteria32 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterEqual ;
  dfs:hasFirstArgument atlas:physicalQuantity30 ;
  dfs:hasSecondArgument atlas:number31 .

atlas:physicalQuantity33 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:PV1 ;
  dfs:refersToFinalStateObjectProperty dfs:pT .

atlas:number34 rdf:type qudt:QuantityValue ;
  qudt:numericValue "1"^^xsd:decimal ;
  qudt:unit unit:GigaElectronVolt .

atlas:selectionCriteria35 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:greaterThan ;
  dfs:hasFirstArgument atlas:physicalQuantity33 ;
  dfs:hasSecondArgument atlas:number34 .

atlas:functionQuantity36 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J1 , atlas:FuncArgNTrack ;
  dfs:hasQuantity "NTrack(J1, DR=0.2, pT>1)" .

atlas:number37 rdf:type qudt:QuantityValue ;
  qudt:numericValue "0"^^xsd:decimal .

atlas:selectionCriteria38 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:equal ;
  dfs:hasFirstArgument atlas:functionQuantity36 ;
  dfs:hasSecondArgument atlas:number37 .

atlas:functionQuantity39 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:J2 , atlas:FuncArgNTrack ;
  dfs:hasQuantity "NTrack(J2, DR=0.2, pT>1)" .

atlas:number40 rdf:type qudt:QuantityValue ;
  qudt:numericValue "0"^^xsd:decimal .

atlas:selectionCriteria41 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:equal ;
  dfs:hasFirstArgument atlas:functionQuantity39 ;
  dfs:hasSecondArgument atlas:number40 .

atlas:physicalQuantity42 rdf:type dfs:PhysicalQuantity ;
  dfs:refersToObject atlas:ETMiss ;
  dfs:refersToFinalStateObjectProperty dfs: .

atlas:number43 rdf:type qudt:QuantityValue ;
  qudt:numericValue "50"^^xsd:decimal ;
  qudt:unit unit:GigaElectronVolt .

atlas:selectionCriteria44 rdf:type dfs:SelectionCriteria ;
  dfs:usesBinaryRelation dfs:lessThan ;
  dfs:hasFirstArgument atlas:physicalQuantity42 ;
  dfs:hasSecondArgument atlas:number43 .

atlas:andor45 rdf:type dfs:And ;
  dfs:hasOperand atlas:selectionCriteria2 , atlas:selectionCriteria5 , atlas:selectionCriteria8 , atlas:selectionCriteria11 ,
  atlas:selectionCriteria14 , atlas:selectionCriteria17 , atlas:selectionCriteria20 , atlas:selectionCriteria23 , 
  atlas:selectionCriteria26 , atlas:selectionCriteria29 , atlas:selectionCriteria32 , atlas:selectionCriteria35 , 
  atlas:selectionCriteria38 , atlas:selectionCriteria41 , atlas:selectionCriteria44 .

atlas:detectorFinalState46 rdf:type dfs:DetectorFinalState ;
  dfs:hasSelectionCriteria atlas:andor45 .



```

# Development

The environment for devleopment is easy to setup:

1. Use Visual Stuido 2015 Community Edition (Free). It may work with older versions, like 2013, but primary development isn't done there.

2. Install the Java Runtime. No need for browser integration. This is needed for the ANTLR code, which uses java to produce the parser.

