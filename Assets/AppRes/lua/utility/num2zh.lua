--- number to chinese string
--- https://blog.csdn.net/ArisKing/article/details/99209292

local function arab_to_chinese(number)
    assert(tonumber(number), "not a number")
    local numerical_tbl = {}
    local numerical_names = {[0] = "零", "一", "二", "三", "四", "五", "六", "七", "八", "九"}
    local numerical_units = {"", "十", "百", "千", "万", "十", "百", "千", "亿", "十", "百", "千", "兆", "十", "百", "千"}

    --01，数字转成表结构存储
    local numerical_length = string.len(number)
    for i = 1, numerical_length do
        numerical_tbl[i] = tonumber(string.sub(number, i, i))
    end

    --02，对应数字转中文处理
    local result_numberical = ""
    local to_append_zero, need_filling = false, true
    for index, number in ipairs(numerical_tbl) do
        --从高位到底位的顺序数字转成对应的从低位到高位的顺序数字单位.
        local real_unit_index = numerical_length - index + 1
        if number == 0 then
            if need_filling then
                if real_unit_index == 5 then--万位
                    result_numberical = result_numberical .. "万"
                    need_filling = false
                end
                if real_unit_index == 9 then--亿位
                    result_numberical = result_numberical .. "亿"
                    need_filling = false
                end
                if real_unit_index == 13 then--兆位
                    result_numberical = result_numberical .. "兆"
                    need_filling = false
                end
            end
            to_append_zero = true
        else
            if to_append_zero then
                result_numberical = result_numberical .. "零"
                to_append_zero = false
            end
            result_numberical = result_numberical  .. numerical_names[number] .. numerical_units[real_unit_index]
            if real_unit_index == 5 or real_unit_index == 9 or real_unit_index == 13 then
                need_filling = false
            else
                need_filling = true
            end
        end
    end
    return result_numberical
end

return arab_to_chinese
