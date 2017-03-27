/**
	PlayerCommandPrototype :
		Prototype of an Player Command
		@date: 09/09/2013

		@Notes:
			- Empty AuthorizedSubCommands List = Already use main(Client, Params)!
			- Set AccessLevel with setAccessLevel(level)
			- Functions of actions must be named in lower case!

		@FileFormat:
			player_command_*.tera.js
				=> * : commandName
**/

///Variables
var Logger = Tera.Libs.Logger;

///Constructor
function construct(Obj){
	//setAccessLevel(0);//Set AccessLevel
	//setAuthorizedSubCommands();//Note : Empty AuthorizedSubCommands List = Already use main(Client, Params)!
}

///Main function without params
function main(Client){
}

///Main function with params
function main(Client, CommandParams)
{
	SendSuccessMessage(Client, "Commande execut�e, main appel�!");
}
