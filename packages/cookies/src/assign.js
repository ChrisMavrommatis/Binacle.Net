export default function (target) {
    for (let i = 1; i < arguments.length; i++) {
        const source = arguments[i];
        for (const key in source) {
            target[key] = source[key]
        }
    }
    return target
}