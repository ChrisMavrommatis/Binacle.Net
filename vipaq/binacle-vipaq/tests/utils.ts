import {faker} from '@faker-js/faker';
import {Bin, Item} from "../src/models";

export function createFakeBin(min: number, max: number) {
	const bin = new Bin();
	bin.length = faker.number.int({min: min, max: max});
	bin.width = faker.number.int({min: min, max: max});
	bin.height = faker.number.int({min: min, max: max});
	return bin;
}

export function createFakeItem(minDimension: number, maxDimension: number, minCoordinate: number, maxCoordinate: number) {
	const item = new Item();
	item.length = faker.number.int({min: minDimension, max: maxDimension});
	item.width = faker.number.int({min: minDimension, max: maxDimension});
	item.height = faker.number.int({min: minDimension, max: maxDimension});
	item.x = faker.number.int({min: minCoordinate, max: maxCoordinate});
	item.y = faker.number.int({min: minCoordinate, max: maxCoordinate});
	item.z = faker.number.int({min: minCoordinate, max: maxCoordinate});
	return item;
}

export function createFakeItems(minDimension: number, maxDimension: number, minCoordinate: number, maxCoordinate: number, noOfItems: number) {
	const items = [];
	for(let i in [...Array(noOfItems).keys()]) {
		items.push(
			createFakeItem(minDimension, maxDimension, minCoordinate, maxCoordinate)
		);
	}

	return items;
}
