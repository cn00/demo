local arr1 = { "", " thousand", " million", " billion" };
local arr2 = {
    "zero",
    "ten",
    "twenty",
    "thirty",
    "forty",
    "fifty",
    "sixty",
    "seventy",
    "eighty",
    "ninety"
};
local arr3 = {
    "zero",
    "one",
    "two",
    "three",
    "four",
    "five",
    "six",
    "sever",
    "eight",
    "nine"
};
local arr4 = {
    "ten",
    "eleven",
    "twelve",
    "thirteen",
    "fourteen",
    "fifteen",
    "sixteen",
    "seventeen",
    "eighteen",
    "nineteen"
};

local function English(num)
    num = tostring(num)
    local strRet = "";
    if (#num == 3 and string.sub(num, 1+0, 3) ~= "000") then
        if (string.sub(num, 1+0, 1) ~= "0") then
            strRet = strRet .. arr3[1+tonumber(string.sub(num, 1+0, 1))] + " hundred";
            if (string.sub(num, 1+1, 1+2) ~= "00") then
                strRet = strRet .. " and ";
            end
        end
        num = string.sub(num, 1+1);
    end
    if (#num == 2) then
        if (string.sub(num, 1+1, 1+1) == "0") then
            num = string.sub(num, 1+1);
        elseif (string.sub(num, 1+0, 1+1) == "1") then
            local arr4i = string.sub(num, 1+1, 1+2)
            print("arr4i", num, arr4i, #arr4, arr3[tonumber(string.sub(num, 1+0, 1+1))])
            strRet = strRet .. arr4[1+tonumber(string.sub(num, 1+1, 1+2))];
        else
            local arri = string.sub(num, 1+1, 1+2)
            print("arr2i", num, arri, #arr2, arr2[tonumber(string.sub(num, 1+0, 1+1))])
            strRet = strRet .. arr2[1+tonumber(string.sub(num, 1+0, 1+1))];
            if (string.sub(num, 1, 1) ~= "0") then
                strRet = strRet .. "-";
            end
            num = string.sub(num, 1);
        end
    end
    if (#num == 1 and string.sub(num, 0, 1) ~= "0") then
        --local arr3i = string.sub(num, 0, 1)
        --print("arr3i", arr3i, #arr3, arr3[tonumber(string.sub(num, 0, 1))])
        strRet = strRet .. arr3[1+tonumber(string.sub(num, 1+0, 1+1))];
    end
    return strRet;
end

local function Translate(num)
    num = tostring(num)
    local len, i, j = #num, 0, 0
    local strRet = "";
    local cols = math.ceil(len / 3);
    local first = len - cols * 3;
    local strRet,num3, strEng = "";
    for i = first, len, 3 do
        j = 1 + j;
        if (i >= 0) then
            num3 = string.sub(num, i, i + 3);
        else
            num3 = string.sub(num, 0, first + 3);
        end
        strEng = English(num3);
        if (strEng ~= "") then
            if (strRet ~= "") then
                strRet = strRet .. ",";
            end
            local arri = 1+(cols - j)
            print("arr1i", num3, arri, #arr1, arr1[arri], strRet)
            strRet = strRet .. English(num3) .. arr1[1+(cols - j)];
        end
    end
    return strRet;
end -- Translate

return {
    Translate = Translate,
    English = English,
}