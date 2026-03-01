window.pickdriverRaces = {
    getBrowserTimeZone: function () {
        try {
            return Intl.DateTimeFormat().resolvedOptions().timeZone || null;
        } catch {
            return null;
        }
    },
    getRowEndIndex: function (element, index) {
        if (!element) {
            return index || 0;
        }

        var cards = Array.prototype.slice.call(element.querySelectorAll(".race-card"));
        if (!cards.length) {
            return index || 0;
        }

        var targetIndex = typeof index === "number" ? index : 0;
        var target = cards[targetIndex];
        if (!target) {
            return cards.length - 1;
        }

        var targetTop = target.offsetTop;
        var rowEnd = targetIndex;
        for (var i = targetIndex + 1; i < cards.length; i++) {
            var top = cards[i].offsetTop;
            if (top === targetTop) {
                rowEnd = i;
            } else {
                break;
            }
        }

        return rowEnd;
    },
    scrollToElement: function (element, offset) {
        if (!element) {
            return;
        }

        var rect = element.getBoundingClientRect();
        var top = rect.top + window.scrollY;
        var y = top - (offset || 0);
        window.scrollTo({ top: y, behavior: "smooth" });
    }
};
