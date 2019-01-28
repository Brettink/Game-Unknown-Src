using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GS {
	L, N, E, I, C
}

public enum ES {
	SS, CS, LB, HB
}

public enum Type{
	Equip, Heal
}

public enum EType{
	None, headS, chestS, weapon, legS, footS
}

public enum WType{ slotL, slotR }

public enum STAT{
	STR, AGI, DEX, END, CON, WIS, INT, CHA, LUC, XPG, HEH, MAH
}

public enum NPCType{
	goblin
}

enum STATE{
	NONE, LERPING, MOVING_SELF, MOVING_PANEL
}

public enum DIR {
	Horizontal, Vertical
}
