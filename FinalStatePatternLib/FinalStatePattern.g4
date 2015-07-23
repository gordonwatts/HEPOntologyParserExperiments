grammar FinalStatePattern;

/*
 * Parser Rules
 */

top_level
    : (line_needing_eof LEOF) + 
    ;

line_needing_eof
    : object_specification
    ;

object_specification
	: object_name							# ObjectSpecNameOnly
    | object_name COLON self_ref_cut_list	# ObjectSpecNameAndCutList
    ;

object_name
    : NAME 
	| NAME '(' base_definition ')'
    ;

base_definition
	: NAME
	;

self_ref_cut_list
    : self_ref_cut (',' self_ref_cut)*
    ;

self_ref_cut
    : self_ref_cut_arg BINARY_OP self_ref_cut_arg								# CutBinary
	| cut_number BINARY_OP self_ref_cut_name BINARY_OP cut_number	# CutRange
    ;

self_ref_cut_arg
	: cut_number
	| self_ref_cut_name
	;

self_ref_cut_name
	: NAME
	;

cut_number
	: NUMBER
	| NUMBER unit
	;

unit
	: NAME
	;

/*
 * Lexer Rules
 */

NAME
    : [a-zA-Z][a-zA-Z0-9_-]+
    ;

NUMBER
	: [0-9]+
	| [0-9]* '.' [0-9]+
	;

BINARY_OP
	: '>' | '>='
	| '<' | '<='
	| '=' | '!='
	;

LEOF
    : ';'
    ;

COLON
    : ':'
    ;

WS
    :   (' ' | '\r' | '\n') -> channel(HIDDEN)
    ;
