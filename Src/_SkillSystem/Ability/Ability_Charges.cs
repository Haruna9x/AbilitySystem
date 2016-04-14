﻿using System;
using UnityEngine;
using System.Collections;

public partial class Ability2 {

	private int chargeCount;	//since we use Activator to create instances here need a way to track this on creation
	private int nextCharge;
	private Charge[] charges;

	public bool OnCooldown {
		get {
			return charges[nextCharge].OnCooldown;
		}
	}

	public int ChargeCount {
		get { 
			return charges.Length - 1;
		}
	}

	public int GetCharges(ref Charge[] input, int count = -1) {
		if(count <= 0) {
			count = charges.Length - 1;
		} 
		if(input.Length < charges.Length) {
			Array.Resize(ref input, charges.Length);
		}
		Array.Copy(charges, input, count);
		return count;
	}

	public Charge[] GetCharges() {
		Charge[] output = new Charge[charges.Length];
		Array.Copy(charges, output, charges.Length);
		return output;
	}


	public void AddCharge(float cooldown, bool ready = true) {
		Array.Resize(ref charges, charges.Length + 1);
		charges[charges.Length - 1] = new Charge(cooldown, ready);
	}

	public void SetChargeCooldown(float cooldown) {

	}

	public bool SetChargeCooldown(int chargeIndex, float cooldown) {
		return false;
	}

	public bool RemoveCharge() {
		if(charges.Length == 1) {
			return false;
		} else {
			return true;
		}
	}

	public bool RemoveCharge(int index) {
		if(index >= charges.Length) {
			return false;
		}
		return true;
	}

	public void ExpireCharge() { 
		charges[nextCharge].lastUsed = Timer.GetTimestamp;
		nextCharge = (nextCharge + 1) % charges.Length;
	}

	public bool ExpireCharge(int chargeIndex) {
		return false;
	}

}

