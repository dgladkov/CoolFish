UsedRaft = nil;
local name = nil; 

local hasBuff = function(id)
	local spell = GetSpellInfo(id);
	_,_,_,_,_,_,expires = UnitBuff("player", spell);
	if not expires or expires-GetTime() < 30 then
		return false;
	end
	return true;
end

if hasBuff(124036) or hasBuff(546) or hasBuff(3714) or hasBuff(5697) or hasBuff(1706) then
	return;
end

-- See if we have the water walking raft and use it instead of spells
for i=0,4 do 
	local numberOfSlots = GetContainerNumSlots(i); 
	for j=1,numberOfSlots do 
		local itemId = GetContainerItemID(i,j) 
		if itemId == 85500 then -- The raft item id
			UseContainerItem(i, j);
			UsedRaft = 1;
			return;
		end 
	end 
end

-- If we don't have the raft, check to see if we have a water walking spell instead
local _, englishClass = UnitClass("player") 

if englishClass == "SHAMAN" then 
	name = GetSpellInfo(546)
elseif englishClass == "DEATHKNIGHT" then 
	name = GetSpellInfo(3714) 
elseif englishClass == "WARLOCK" and GetSpecialization() == 1 then
    if not UnitChannelInfo("player") then 
		if UnitPower("player",7) < 1  then
			  name= GetSpellInfo(101976)
			  CastSpellByName(name)
		else
		  name = GetSpellInfo(74434);
		  _, duration= GetSpellCooldown(74434)
		  if duration <= 0 then
		  	  name = GetSpellInfo(5697)
		  end
		end
	  end
elseif englishClass == "PRIEST" then
	  name = GetSpellInfo(1706)
end

if name then
	  TargetUnit("player")
	  CastSpellByName(name)
	  TargetLastTarget()
	  UsedRaft = 1
end