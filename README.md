# HEPOntologyParserExperiments
Experiments with a DSL that will parse to a FinalStateObject OWL

# DSL

The DSL is simple. For example, this specifies an exotics search for displaced jets (http://arxiv.org/abs/1501.04020).

```
   J1 (anti-kt-0.4): et>60 GeV, |eta| < 2.5, EMF < X, -1 ns < ArrivalTime < 5 ns
   J2 (anti-kt-0.4): et>40 GeV, |eta| < 2.5, EMF < X, -1 ns < ArrivalTime < 5 ns
   Veto: J3 (anti-kt-0.4): et>40 GeV, |eta| < 2.5, EMF < X, -1 ns < ArrivalTime < 5 ns
   PV1 (primary vertex): NTrack  >= 3, pT>1 GeV
   NTrack(J1, DR=0.2, pT>1) = 0
   NTrack(J2, DR=0.2, pT>1) = 0
   ETMiss (atlas-ref) < 50 GeV
```
