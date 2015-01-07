AppliedBait = nil;
if not BaitSpellId or not BaitItemId then
	return;
end

if BaitSpellId ~= 1 then
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
                       break;
			    end 
		    end 
	    end
    end
else

    local baitItemSpellMap = {[110293]=158038, [110294]=158039, [110290]=158035, [110289]=158034, [110291]=158036, [110274]=158031, [110292]=158037};
	-- Check to see if we have any of the bait spell buffs
	for index, value in pairs(baitItemSpellMap) do 
	    local name = GetSpellInfo(value);  
		local _,_,_,_,_,_,expires = UnitBuff("player", name);
        if expires then 
	        expires = expires-GetTime();
			-- If the current hasn't expired yet, just return
	        if expires > 10 then
    	        return;
	        end
        end
	end
    -- Check to see if we have any baits in our bags
		local foundBaits = {};
	    for i=0,4 do 
		    local numberOfSlots = GetContainerNumSlots(i); 
		    for j=1,numberOfSlots do 
			    local itemId = GetContainerItemID(i,j) 
			    if baitItemSpellMap[itemId] then
					   table.insert(foundBaits, itemId);
			    end 
		    end 
	    end
	-- If we find baits in our bags, pick one at random
		if table.getn(foundBaits) ~= 0 then
			local id = foundBaits[math.random(#foundBaits)];
			local baitName = GetItemInfo(id);
			RunMacroText("/use " .. baitName);
			AppliedBait = 1;
		end
end