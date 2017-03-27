/**
	JSIAMind_17 :
		IA Mind of Dopeul::"Iop", Dopeul::"Cra"
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
				SelfAction();
				break;
			}
			case 2:{
				InitCells();
				SelfAction();
				break;
			}
			case 3:{
				InitCells();
				SelfAction();
				break;
			}
			case 4:{
				InitCells();
				SelfAction();
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
