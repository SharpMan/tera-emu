/**
	JSIAMind_18 :
		IA Mind of "Prespic"
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
				Support();
				break;
			}
			case 2:{
				InitCells();
				Attack();
				break;
			}
			case 3:{
				InitCells();
				MoveFar(4);
				break;
			}
			case 4:{
				InitCells();
				MoveFar(4);
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
