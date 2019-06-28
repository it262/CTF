using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData{

    public string id, name;
    public bool isPlayer = false;

    public int atk;
	public int dif;
	public int spd;
	public int life;

    private int max;
    private int min;

    private int plus = 3;
    private int minus = 1;

    public float attackRange = 5f;

    public PlayerData(){
		atk = 50;
		dif = 50;
		spd = 50;
		life = 50;
	}

    public void changeState(string str)
    {
        switch (str) {
				case "atk":
					atk_plus ();
					break;
				case "dif":
					dif_plus ();
					break;
				case "spd":
					spd_plus ();
					break;
				default:
					black_minus();
					break;
				}
    }

    public void atk_plus() {
        atk += plus;
        dif = (dif - minus <= 0) ? min : dif - minus;
        spd = (spd - minus <= 0) ? min : spd - minus;
    }
	public void dif_plus(){
		dif += plus;
		atk = (atk - minus <= 0) ? min : atk - minus;
		spd = (spd - minus <= 0) ? min : spd - minus;
	}
	public void spd_plus(){
		spd += plus;
		atk = (atk - minus <= 0) ? min : atk - minus;
		dif = (dif - minus <= 0) ? min : dif - minus;
	}

    public void black_minus() {
        int sub = Random.Range(0, minus);
        spd = (spd - sub <= 0) ? min : spd - sub;
        sub = Random.Range(0, minus);
        atk = (atk - sub <= 0) ? min : atk - sub;
        sub = Random.Range(0, minus);
        dif = (dif - sub <= 0) ? min : dif - sub;
    }

    public float getMoveSpeed()
    {
        return 1 + spd * 0.1f;
    }

    public float getAttackSpeed()
    {
        return 1 / spd;
    }

    public Vector3 getImpactVector(Vector3 vector,PlayerData target)
    {
        return vector * ((atk-target.dif <= 0)? 1: atk - target.dif);
    }


}
