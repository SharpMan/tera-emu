/**
	JSCellActionPrototype :
		Prototype of a scripted CellAction
		@date: 05/06/2013
**/

///Variables
var Logger = Tera.Libs.Logger;
var MapTable = Tera.WorldServer.Database.Tables.MapTable;

///Constructor
function construct(){}

///CellActionCache for cellAction inst.
function CellActionCache(mapId, cellId, targetMapId, targetCellId){
	this.mapId = mapId;
	this.cellId = cellId;
	this.targetMapId = targetMapId;
	this.targetCellId = targetCellId;
}

///Make CellActionCache for current cellAction inst.
var constructCache = function (mapId, cellId, args){
	var targetMap = parseInt(args.split(',')[0]);
	var targetCell = parseInt(args.split(',')[1]);
	return new CellActionCache(mapId, cellId, targetMap, targetCell);
}

///Test parameters before call Apply function
var canApply = function(cellActionCache, triggerPlayer){
	return true;
}

///Apply
function apply(cellActionCache, triggerPlayer){
	var map = MapTable.Get(cellActionCache.targetMapId);
	if(map!=null){
			Logger.Info("Cells count : " + (map.mapData.Length/10));
	}
}
