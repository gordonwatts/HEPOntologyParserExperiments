grammar FinalStatePattern;

/*
 * Parser Rules
 */

top_level
    : (line_needing_eof LEOF) + 
    ;

line_needing_eof
    : object_specification
	| standalone_cut
    ;

standalone_cut
	: cut
	;

object_specification
	: object_name						# ObjectSpecNameOnly
    | object_name COLON cut_list		# ObjectSpecNameAndCutList
    ;

object_name
    : NAME 
	| NAME '(' base_definition ')'
    ;

base_definition
	: NAME
	;

cut_list
    : cut (',' cut)*
    ;

cut
    : cut_arg BINARY_OP cut_arg								# CutBinary
	| cut_number BINARY_OP cut_name BINARY_OP cut_number	# CutRange
    ;

cut_arg
	: cut_number
	| cut_name
	| function
	;

function
	: NAME '(' (function_arg (',' function_arg)*)? ')'
	;

function_arg
	: cut_name
	| cut
	;

cut_name
	: NAME
	| object_name
	| object_name '.' NAME
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
	: ('-')? [0-9]+
	| ('-')? [0-9]* '.' [0-9]+
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
