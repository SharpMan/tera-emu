/**
	JSIAMind_13 :
		IA Mind of "Tonneau"
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
		var hasState = myFighter.States.HasState(Tera.WorldServer.World.Fights.FighterStateEnum.STATE_PORTE);
		switch (UsedNeurons)
        {
			case 1:{
				InitCells();
				if(hasState){
					Heal();
				}else{
					Repels();
				}
				break;
			}
			case 2:{
				InitCells();
				if(hasState){
					Heal();
				}else{
					Repels();
				}
				break;
			}
			default:{
				Stop();
				break;
			}
		}
	}
}
