Some notes:

# Restructering how the listener/visitor works

  See http://stackoverflow.com/questions/29929062/antlr4-ast-vistor-and-node-return-type-should-i-have-more-than-one-visitor - the point is there
  isn't anything that will get us back our Sprache code. So this is something we have to give up. We either have to add annotations to the tree
  as we move through it, or we have to generate a giant single class. Both very ugly.

  They really should take a change to allow one to specify return types.