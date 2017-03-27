/**
	JSIAMind_10 :
		IA Mind of "Explosive"
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
				Attack();
				break;
			}
			case 2:{
				InitCells();
				Buff();
				break;
			}
			case 3:{
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
