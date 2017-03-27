/**
	JSIAMind_21 :
		IA Mind of "Tofu"
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
				Attack();
				break;
			}
			case 3:{
				InitCells();
				Attack();
				break;
			}
			case 4:{
				InitCells();
				MoveFar();
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
