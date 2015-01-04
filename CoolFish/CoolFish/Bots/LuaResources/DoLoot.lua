if IsFishingLoot() then 
	local numLootItems = GetNumLootItems()
	for i=1,numLootItems do 
	local Link = GetLootSlotLink(i)
	local _,name, lootQuantity ,Quality  = GetLootSlotInfo(i)
	
		if Link and Quality and name and lootQuantity then

		_, _, id = string.find(Link, "item:(%d+):")
			if Quality >= LootQuality then
				if LootLeftOnly or DontLootLeft then 
					match = ItemsList[name] or ItemsList[id]
                
					if (match) and LootLeftOnly then 
						LootSlot(i) 
						ConfirmLootSlot(i)
						if LootLog[id] then
							LootLog[id] = LootLog[id] + lootQuantity;
						else
							LootLog[id] = lootQuantity;
						end
						if DODEBUG then
							print("Trying to Loot: " .. Link);
						end                
					else
						if (not match) and DontLootLeft then
							LootSlot(i) 
							ConfirmLootSlot(i)
							if LootLog[id] then
								LootLog[id] = LootLog[id] + lootQuantity;
							else
								LootLog[id] = lootQuantity;
							end
							if DODEBUG then
								print("Trying to Loot: " .. Link);
							end
						else
							if DODEBUG then
								print("NOT Looting " .. Link);
							end
							if NoLootLog[id] then
								NoLootLog[id] = NoLootLog[id] + lootQuantity;
							else
								NoLootLog[id] = lootQuantity;
							end
						end
					end 
				else 
						LootSlot(i) 
						ConfirmLootSlot(i)
						if LootLog[id] then
							LootLog[id] = LootLog[id] + lootQuantity;
						else
							LootLog[id] = lootQuantity;
						end 
						if DODEBUG then
							print("Trying to Loot: " .. Link);
						end
                        
				end 
			else
				if DODEBUG then
					print(Link .. " didn't match Loot Quality Minimum. Quality: " .. Quality .. " Min: " .. LootQuality);
				end

				if NoLootLog[id] then
					NoLootLog[id] = NoLootLog[id] + lootQuantity;
				else
					NoLootLog[id] = lootQuantity;
				end
			end
		end
	end
	CloseLoot();
	StateResult = 1;
end;
StateResult = 0;