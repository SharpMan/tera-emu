/**
	JSIAMind_0 :
		Passive IA Mind
		@date: 08/06/2013
**/

///Variables
var Logger = Tera.Libs.Logger;

///Constructor
function construct(){}

///When a virtual fighter must play
function Play(IA)
{
	with(IA){
		Wait(250);
		Stop();
	}
}
