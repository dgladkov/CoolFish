-- Check to see if the buff has expired already
AppliedBait = nil;
if not BaitSpellId or not BaitItemId then
	return;
end

local name = GetSpellInfo(BaitSpellId);  
local _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
	expires = expires-GetTime();
	if expires <= 10 then
		expires = true
	else
		expires = false
	end
else
	expires = true
end

if expires then
	-- Check to see if we have any baits in our inventory
	for i=0,4 do 
		local numberOfSlots = GetContainerNumSlots(i); 
		for j=1,numberOfSlots do 
			local itemId = GetContainerItemID(i,j) 
			if itemId == BaitItemId then 
				   local BaitName = GetItemInfo(itemId);
				   RunMacroText("/use " .. BaitName);
				   AppliedBait = 1;
			end 
		end 
	end
end