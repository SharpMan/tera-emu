/**
	PlayerCommandPrototype :
		Prototype of an Player Command
		@author: alleos13
		@date: 09/09/2013
		
		@Notes:
			- Empty AuthorizedSubCommands List = Already use main(Client, Params)!
			- Set AccessLevel with setAccessLevel(level)
			- Functions of actions must be named in lower case!
			- add authorized subaction with addAuthorizedSubCommand(name)
			
		@FileFormat:
			player_command_[*].tera.js 
				=> * : commandName
**/

///Variables
var Logger = Tera.Libs.Logger;
	
///Constructor
function construct(){
	setAccessLevel(5);//Set AccessLevel
	addAuthorizedSubCommand("test");//Note : Empty AuthorizedSubCommands List = Already use main(Client, Params)!
}

///Main function with params
function main(Client, CommandParams)
{
}

///a Test Command
function test(Client, CommandParams){
	
}