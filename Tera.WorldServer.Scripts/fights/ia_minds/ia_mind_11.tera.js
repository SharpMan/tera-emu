/**
	JSIAMind_11 :
		IA Mind of "Chafer"
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
		switch (UsedNeurons)
        {
			case 1:{
				InitCells();
				MadSelfAction();
				break;
			}
			case 2:{
				InitCells();
				MadSelfAction();
				break;
			}
			case 3:{
				InitCells();
				MadSelfAction();
				break;
			}
			case 4:{
				InitCells();
				MadSelfAction();
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
