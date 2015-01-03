LureName = nil;

local Lures = {
	-- Consumable Lures
	[67404] = true,
	[6529] = true,
	[6811] = true,
	[6530] = true,
	[6532] = true,
	[7307] = true,
	[6533] = true,
	[62673] = true,
	[46006] = true,
	[34861] = true,
	[68049] = true,
	[118391] = true; -- Worm Supreme
	-- Hats
	[33820] = true, -- Weather
	[117405] = true, -- Nat's Drinking Hat
	[88710] = true, -- Nat's Hat
	-- Poles
	[116826] = true, -- Draenic Fishing Pole
	[116825] = true -- Savage Fishing Pole
}

-- Check to see if we have any lures in our inventory
for i=0,4 do 
    local numberOfSlots = GetContainerNumSlots(i); 
    for j=1,numberOfSlots do 
	    local itemId = GetContainerItemID(i,j) 
        if Lures[itemId] then 
               LureName = GetItemInfo(itemId);
			   return;
        end 
    end 
end

-- Function to find inventory slot lures
local checkLure = function(slot)
    local ItemID = GetInventoryItemID("player", slot);
	if Lures[ItemID]   then 
		local start, duration = GetInventoryItemCooldown("player", slot);
		if start+duration-GetTime() < 0 then 
			return ItemID;
		end 
	end
	return nil
end

-- Check to see if we have any hat or weapon lures
local headLure = checkLure(1);
local weaponLure = checkLure(16);

if weaponLure then
	LureName = GetItemInfo(weaponLure);
	return;
end

if headLure then
	LureName = GetItemInfo(headLure);
	return;
end
