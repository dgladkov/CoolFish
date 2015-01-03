-- Check to see if the buff has expired already
AppliedBait = nil;
if not BaitSpellId or not BaitItemId then
	return;
end

if BaitSpellId ~= 999999 then
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
    local baitSpellItemMap = {[158038]=110293, [158039]=110294, [158035]=110290, [158034]=110289, [158036]=110291, [158031]=110274, [158037]=110292};
    local baitItemSpellMap = {[110293]=158038, [110294]=158039, [110290]=158035, [110289]=158034, [110291]=158036, [110274]=158031, [110292]=158037};
    local applyBait = true;
    for count=1,40 do 
        local _,_,_,_,_,_,expires,_,_,_,spellID = UnitBuff("player",count);
        if baitSpellItemMap[spellID] then
            if expires then 
	            expires = expires-GetTime();
	            if expires > 10 then
    	            applyBait = false
	            end
            end
        end
    end
    if applyBait then
	    -- Apply the first bait found in our inventory
	    for i=0,4 do 
		    local numberOfSlots = GetContainerNumSlots(i); 
		    for j=1,numberOfSlots do 
			    local itemId = GetContainerItemID(i,j) 
			    if baitItemSpellMap[itemId] then 
				       local BaitName = GetItemInfo(itemId);
				       RunMacroText("/use " .. BaitName);
				       AppliedBait = 1;
                       break;
			    end 
		    end 
	    end
    end
end