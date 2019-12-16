name = "train"
-- whitelists for profile and meta
profile_whitelist = {
    "railway"
}
meta_whitelist = {
    "name"
}
-- profile definitions linking a function to a profile
profiles = {
    { 
        name = "shortest",
        function_name = "factor_and_speed",
        metric = "distance",
    }
}
-- the main function turning attributes into a factor_and_speed and a tag whitelist
function factor_and_speed (attributes, result)

     result.speed = 0
     result.direction = 0
     result.canstop = true
     result.attributes_to_keep = {}

     -- get default speed profiles
     local railway = attributes.railway
     if railway == "light_rail" or
		railway == "monorail" or
		railway == "narrow_gauge" or
		railway == "rail" or
		railway == "subway" or
		railway == "tram" or
		railway == "miniature" then
        result.speed = 50 -- speed in km/h
        result.direction = 0
        result.canstop = true
        result.attributes_to_keep.railway = railway
    end
end