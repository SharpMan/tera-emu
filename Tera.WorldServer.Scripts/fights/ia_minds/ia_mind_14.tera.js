/**
	JSIAMind_14 :
		IA Mind of "Surpuissante"
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
				Invocate();
				break;
			}
			case 2:{
				InitCells();
				Invocate();
				break;
			}
			case 3:{
				InitCells();
				moveFar();
				break;
			}
			case 4:{
				InitCells();
				moveFar();
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
