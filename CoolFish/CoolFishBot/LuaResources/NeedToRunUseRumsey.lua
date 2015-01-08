UsedRumsey = nil;
local name = GetSpellInfo(45694); 
_,_,_,_,_,_,expires = UnitBuff("player", name); 


if expires then 
	expires = expires-GetTime()
	if expires > 30 then
		return;
	end
end

for i=0,4 do 
	local numberOfSlots = GetContainerNumSlots(i); 
	for j=1,numberOfSlots do 
		local itemid = GetContainerItemID(i,j)
		if itemid == 34832 then
				name = GetItemInfo(34832); 
				RunMacroText("/use  " .. name);
				UsedRumsey = 1;
				return;
		end        
	end 
end