/**
	JSIAMind_5 :
		Blocker IA Mind
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
				moveToEnnemy();
				break;
			}
			case 2:{
				InitCells();
				moveToEnnemy();
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
