/**
	JSIAMindPrototype :
		Prototype of a IA Mind
		@date: 08/06/2013
**/

///Variables
var Logger = Tera.Libs.Logger;

///Constructor
function construct(){}

///When a virtual fighter must play
function Play(IA)
{
	with(IA){//Use all the functions of IA without specifying the reference
		Wait(250);
		Stop();
	}
}
