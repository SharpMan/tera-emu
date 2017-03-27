/**
	JSIAMind_9 :
		IA Mind of "Coffre"
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
				Buff();
				break;
			}
			case 2:{
				InitCells();
				Buff();
				break;
			}
			case 3:{
				InitCells();
				Buff();
				break;
			}
			case 4:{
				InitCells();
				moveFar(4);
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
