using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GS {
	L, N, E, I, C, O
}

public enum ES {
	SS, CS, LB, HB
}
public enum SIDE {
    N, NE, E, SE, S, SW, W, NW
}

public enum EType{
	chestS, legS, headS, footS
}

public enum WType{ slotL, slotR }

public enum STAT{
	STR, AGI, DEX, END, CON, WIS, INT, CHA, LUC, XPG, HEH, MAH
}

public enum EnemyType {
    goblin, archer, mage
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
